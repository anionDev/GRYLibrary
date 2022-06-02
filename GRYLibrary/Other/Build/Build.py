import os
import sys
from pathlib import Path
from lxml import etree
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities


@GeneralUtilities.check_arguments
def dotnet_sign_file(self: ScriptCollectionCore, file: str, keyfile: str):
    directory = os.path.dirname(file)
    filename = os.path.basename(file)
    if filename.lower().endswith(".dll"):
        filename = filename[:-4]
        extension = "dll"
    elif filename.lower().endswith(".exe"):
        filename = filename[:-4]
        extension = "exe"
    else:
        raise Exception("Only .dll-files and .exe-files can be signed")
    self.run_program("ildasm",
                     f'/all /typelist /text /out="{filename}.il" "{filename}.{extension}"', directory)
    self.run_program("ilasm",
                     f'/{extension} /res:"{filename}.res" /optimize /key="{keyfile}" "{filename}.il"', directory)
    os.remove(directory+os.path.sep+filename+".il")
    os.remove(directory+os.path.sep+filename+".res")


@GeneralUtilities.check_arguments
def standardized_tasks_build_for_dotnet_create_package(self: ScriptCollectionCore, repository: str, codeunitname: str, outputfolder: str):
    build_folder = os.path.join(repository, codeunitname, "Other", "Build")
    root: etree._ElementTree = etree.parse(os.path.join(build_folder, f"{codeunitname}.nuspec"))
    version = root.xpath("//*[name() = 'package']/*[name() = 'metadata']/*[name() = 'version']/text()")[0]
    nupkg_filename = f"{codeunitname}.{version}.nupkg"
    nupkg_file = f"{build_folder}/{nupkg_filename}"
    GeneralUtilities.ensure_file_does_not_exist(nupkg_file)
    self.run_program("nuget", f"pack {codeunitname}.nuspec", build_folder)
    GeneralUtilities.ensure_directory_does_not_exist(outputfolder)
    GeneralUtilities.ensure_directory_exists(outputfolder)
    os.rename(nupkg_file, f"{build_folder}/BuildArtifact/{nupkg_filename}")


@GeneralUtilities.check_arguments
def standardized_tasks_build_for_dotnet_build(self: ScriptCollectionCore, csproj_file: str, buildconfiguration: str, outputfolder: str, files_to_sign: dict):
    # TODO update version in csproj-file
    csproj_file_folder = os.path.dirname(csproj_file)
    csproj_file_name = os.path.basename(csproj_file)
    self.run_program("dotnet", f"clean", csproj_file_folder)
    GeneralUtilities.ensure_directory_does_not_exist(outputfolder)
    GeneralUtilities.ensure_directory_exists(outputfolder)
    self.run_program("dotnet", f"build {csproj_file_name} -c {buildconfiguration} -o {outputfolder}", csproj_file_folder)
    for file, keyfile in files_to_sign.items():
        dotnet_sign_file(self, os.path.join(outputfolder, file), keyfile)


@GeneralUtilities.check_arguments
def standardized_tasks_build_for_dotnet_project_in_common_project_structure(self: ScriptCollectionCore, repository_folder: str, codeunitname: str, buildconfiguration: str, build_test_project_too: bool, output_folder: str, commandline_arguments: list[str]):
    codeunit_folder = os.path.join(repository_folder, codeunitname)
    csproj_file = os.path.join(codeunit_folder, codeunitname, codeunitname+".csproj")
    csproj_test_file = os.path.join(codeunit_folder, codeunitname+"Tests", codeunitname+"Tests.csproj")
    commandline_arguments = commandline_arguments[1:]
    files_to_sign: dict() = dict()
    for commandline_argument in commandline_arguments:
        if commandline_argument.startswith("-sign:"):
            commandline_argument_splitted: list[str] = commandline_argument.split(":")
            files_to_sign[commandline_argument_splitted[1]] = commandline_argument[len("-sign:"+commandline_argument_splitted[1]):]
    self.run_program("dotnet", "restore", codeunit_folder)
    standardized_tasks_build_for_dotnet_build(self, csproj_file, buildconfiguration, os.path.join(output_folder, codeunitname), files_to_sign)
    if build_test_project_too:
        standardized_tasks_build_for_dotnet_build(self, csproj_test_file, buildconfiguration, os.path.join(output_folder, codeunitname+"Tests"), files_to_sign)


@GeneralUtilities.check_arguments
def standardized_tasks_build_for_dotnet_library_project_in_common_project_structure(self: ScriptCollectionCore, buildscript_file: str, buildconfiguration: str = "Release", commandline_arguments: list[str] = []):
    repository_folder: str = str(Path(os.path.dirname(buildscript_file)).parent.parent.parent.absolute())
    codeunitname: str = os.path.basename(str(Path(os.path.dirname(buildscript_file)).parent.parent.absolute()))
    for commandline_argument in commandline_arguments:
        if commandline_argument.startswith("-buildconfiguration:"):
            buildconfiguration = commandline_argument[len("-buildconfiguration:"):]
    outputfolder = os.path.join(os.path.dirname(buildscript_file), "BuildArtifact")
    GeneralUtilities.ensure_directory_does_not_exist(outputfolder)
    GeneralUtilities.ensure_directory_exists(outputfolder)
    standardized_tasks_build_for_dotnet_project_in_common_project_structure(self, repository_folder, codeunitname, buildconfiguration, True, outputfolder, commandline_arguments)
    standardized_tasks_build_for_dotnet_create_package(self, repository_folder, codeunitname, outputfolder)


def build():
    standardized_tasks_build_for_dotnet_library_project_in_common_project_structure(ScriptCollectionCore(), str(Path(__file__).absolute()), "QualityCheck", sys.argv)


if __name__ == "__main__":
    build()
