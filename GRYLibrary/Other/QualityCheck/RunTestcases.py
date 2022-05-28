import os
import sys
from pathlib import Path
from lxml import etree
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities


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


def standardized_tasks_build_for_dotnet_create_package(self: ScriptCollectionCore, repository: str, codeunitname: str, outputfolder: str, push: bool, push_options: str):
    # TODO update version in nuspec-file
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
    if push:
        pass  # TODO push using push_options


def standardized_tasks_build_for_dotnet_build(self: ScriptCollectionCore, csproj_file: str, buildconfiguration: str, outputfolder: str, files_to_sign: dict()):
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
def standardized_tasks_build_for_dotnet_executable_project_in_common_project_structure(self: ScriptCollectionCore, repository_folder: str, codeunitname: str, buildconfiguration: str, build_test_project_too: bool, output_folder: str, commandline_arguments: list[str]):
    csproj_file = os.path.join(repository_folder, codeunitname, codeunitname, codeunitname+".csproj")
    csproj_test_file = os.path.join(repository_folder, codeunitname, codeunitname+"Tests", codeunitname+"Tests.csproj")
    commandline_arguments = commandline_arguments[1:]
    files_to_sign: dict() = dict()
    for commandline_argument in commandline_arguments:
        if commandline_argument.startswith("-sign:"):
            commandline_argument_splitted: list[str] = commandline_argument.split(":")
            files_to_sign[commandline_argument_splitted[1]] = commandline_argument_splitted[2]

    standardized_tasks_build_for_dotnet_build(self, csproj_file, buildconfiguration, output_folder, files_to_sign)
    if build_test_project_too:
        standardized_tasks_build_for_dotnet_build(self, csproj_test_file, buildconfiguration, output_folder, files_to_sign)


def generate_coverage_report(self: ScriptCollectionCore, repository_folder: str, codeunitname: str, verbosity: int):
    """This script expects that the file '<repositorybasefolder>/<codeunitname>/Other/QualityCheck/TestCoverage/TestCoverage.xml' which contains a test-coverage-report in the cobertura-format exists.
This script expectes that the testcoverage-reportfolder is '<repositorybasefolder>/<codeunitname>/Other/QualityCheck/TestCoverage/TestCoverageReport'.
This script expectes that a test-coverage-badges should be added to '<repositorybasefolder>/<codeunitname>/Other/QualityCheck/TestCoverage/Badges'."""
    if verbosity == 0:
        verbose_argument_for_reportgenerator = "Off"
    if verbosity == 1:
        verbose_argument_for_reportgenerator = "Error"
    if verbosity == 2:
        verbose_argument_for_reportgenerator = "Info"
    if verbosity == 3:
        verbose_argument_for_reportgenerator = "Verbose"

    # Generating report
    GeneralUtilities.ensure_directory_does_not_exist(os.path.join(repository_folder, codeunitname, "Other/QualityCheck/TestCoverage/TestCoverageReport"))
    GeneralUtilities.ensure_directory_exists(os.path.join(repository_folder, codeunitname, "Other/QualityCheck/TestCoverage/TestCoverageReport"))
    self.run_program("reportgenerator", "-reports:Other/QualityCheck/TestCoverage/TestCoverage.xml -targetdir:Other/QualityCheck/TestCoverage/TestCoverageReport " +
                     f"-verbosity:{verbose_argument_for_reportgenerator}", os.path.join(repository_folder, codeunitname))

    # Generating badges
    GeneralUtilities.ensure_directory_does_not_exist(os.path.join(repository_folder, "Other/QualityCheck/TestCoverage/Badges"))
    GeneralUtilities.ensure_directory_exists(os.path.join(repository_folder, "Other/QualityCheck/TestCoverage/Badges"))
    self.run_program("reportgenerator", "-reports:Other/QualityCheck/TestCoverage/TestCoverage.xml -targetdir:Other/QualityCheck/TestCoverage/Badges -reporttypes:Badges " +
                     f"-verbosity:{verbose_argument_for_reportgenerator}",  os.path.join(repository_folder, codeunitname))


@GeneralUtilities.check_arguments
def standardized_tasks_run_testcases_for_dotnet_project_in_common_project_structure(self: ScriptCollectionCore, runtestcases_file: str, buildconfiguration: str = "Release", commandline_arguments: list[str] = []):
    repository_folder = str(Path(os.path.dirname(runtestcases_file)).parent.parent.parent.absolute())
    codeunit_name = os.path.basename(str(Path(os.path.dirname(runtestcases_file)).parent.parent.absolute()))
    testprojectname = codeunit_name+"Tests"
    output_folder = f"{repository_folder}/{codeunit_name}/Other/Build/BuildArtifact"
    standardized_tasks_build_for_dotnet_executable_project_in_common_project_structure(
        ScriptCollectionCore(), repository_folder, codeunit_name, buildconfiguration, True, output_folder, commandline_arguments)
    coveragefilesource = os.path.join(repository_folder, codeunit_name, testprojectname, "TestCoverage.xml")
    coveragefiletarget = os.path.join(repository_folder, codeunit_name, "Other/QualityCheck/TestCoverage/TestCoverage.xml")
    GeneralUtilities.ensure_file_does_not_exist(coveragefilesource)
    self.run_program("dotnet", f"test {testprojectname}/{testprojectname}.csproj -c {buildconfiguration}"
                     f" --verbosity normal /p:CollectCoverage=true /p:CoverletOutput=TestCoverage.xml"
                     f" /p:CoverletOutputFormat=cobertura", os.path.join(repository_folder, codeunit_name))
    GeneralUtilities.ensure_file_does_not_exist(coveragefiletarget)
    os.rename(coveragefilesource, coveragefiletarget)
    generate_coverage_report(ScriptCollectionCore(), repository_folder, codeunit_name, 1)


def run_testcases():
    standardized_tasks_run_testcases_for_dotnet_project_in_common_project_structure(ScriptCollectionCore(), __file__, "QualityCheck", sys.argv)


if __name__ == "__main__":
    run_testcases()
