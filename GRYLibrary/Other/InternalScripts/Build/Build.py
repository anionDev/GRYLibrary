import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities
import sys

def dotnet_sign_file(self:ScriptCollectionCore,file:str, keyfile:str):
    directory=os.path.dirname(file)
    filename=os.path.basename(file)
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

def standardized_tasks_build_for_dotnet_create_package(self:ScriptCollectionCore,repository:str,codeunitname:str,outputfolder:str,push:bool,push_options:str):
    self.run_program("nuget", f"pack {repository}.nuspec",os.path.join(repository,codeunitname,"Other","InternalScripts","Build"))
    GeneralUtilities.ensure_directory_does_not_exist(outputfolder)
    GeneralUtilities.ensure_directory_exists(outputfolder)
    os.rename(f"{codeunitname}.nuspec","Result/{codeunitname}.nuspec")


    if push:
        pass#TODO push using push_options

def standardized_tasks_build_for_dotnet_build(self:ScriptCollectionCore,codeunitfolder:str,codeunitname:str,buildconfiguration:str,outputfolder:str,files_to_sign:dict()):
    self.run_program("dotnet", f"clean -c {buildconfiguration}", codeunitfolder)
    GeneralUtilities.ensure_directory_does_not_exist(outputfolder)
    GeneralUtilities.ensure_directory_exists(outputfolder)
    self.run_program("dotnet", f"build {codeunitname}/{codeunitname}.csproj -c {buildconfiguration} -o {outputfolder}", codeunitfolder)
    for file, keyfile  in files_to_sign.items():
        dotnet_sign_file(self,os.path.join( outputfolder, file), keyfile)

def standardized_tasks_build_for_dotnet_executable_project_in_common_project_structure(self:ScriptCollectionCore,repository_folder:str,codeunitname:str,buildconfiguration:str,commandline_arguments:list[str]):
    codeunitfolder=os.path.join(repository_folder,codeunitname)
    outputfolder=os.path.join(os.path.dirname(__file__),"Result")
    commandline_arguments=commandline_arguments[1:]
    files_to_sign:dict()=dict()
    for commandline_argument in commandline_arguments:
        if commandline_argument.startswith("-sign:"):
            commandline_argument_splitted:list[str]=commandline_argument.split(":")
            files_to_sign[commandline_argument_splitted[1]]=commandline_argument_splitted[2]
    standardized_tasks_build_for_dotnet_build(self,codeunitfolder,codeunitname,buildconfiguration,outputfolder,files_to_sign)

def standardized_tasks_build_for_dotnet_library_project_in_common_project_structure(self:ScriptCollectionCore,repository_folder:str,codeunitname:str,buildconfiguration:str,commandline_arguments:list[str]):
    standardized_tasks_build_for_dotnet_executable_project_in_common_project_structure(self,repository_folder,codeunitname,buildconfiguration,commandline_arguments)
    outputfolder=os.path.join(os.path.dirname(__file__),"Result")
    push=False
    push_options=""
    for commandline_argument in commandline_arguments:
        if commandline_argument.startswith("-push:"):
            push=True
            push_options=commandline_argument[len("-push:"):]
    standardized_tasks_build_for_dotnet_create_package(self,repository_folder,codeunitname,outputfolder,push,push_options)

def build():
    standardized_tasks_build_for_dotnet_library_project_in_common_project_structure(ScriptCollectionCore(),
        str(Path(os.path.dirname(__file__)).parent.parent.parent.parent.absolute()), "GRYLibrary","Productive", sys.argv)


if __name__ == "__main__":
    build()
