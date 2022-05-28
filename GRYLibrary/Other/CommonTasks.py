import sys
import os
import re
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities

def replace_version_in_nuspec_file(self:ScriptCollectionCore,nuspec_file:str,current_version:str):
        versionregex = "\\d+\\.\\d+\\.\\d+"
        versiononlyregex = f"^{versionregex}$"
        pattern = re.compile(versiononlyregex)
        if pattern.match(current_version):
            GeneralUtilities.write_text_to_file(nuspec_file, re.sub(f"<version>{versionregex}<\\/version>",
                                                                      f"<version>{current_version}</version>", GeneralUtilities.read_text_from_file(nuspec_file)))
        else:
            raise ValueError(f"Version '{current_version}' does not match version-regex '{versiononlyregex}'")

def replace_version_in_csproj_file(self:ScriptCollectionCore,csproj_file:str,current_version:str):
        versionregex = "\\d+\\.\\d+\\.\\d+"
        versiononlyregex = f"^{versionregex}$"
        pattern = re.compile(versiononlyregex)
        if pattern.match(current_version):
            GeneralUtilities.write_text_to_file(csproj_file, re.sub(f"<Version>{versionregex}<\\/Version>",
                                                                      f"<Version>{current_version}</Version>", GeneralUtilities.read_text_from_file(csproj_file)))
        else:
            raise ValueError(f"Version '{current_version}' does not match version-regex '{versiononlyregex}'")

def common_tasks():
    file=Path(__file__).absolute()
    folder_of_current_file=os.path.dirname(file)
    sc=ScriptCollectionCore()
    version=sc.getversion_from_arguments_or_gitversion(file,sys.argv)
    sc.update_version_of_codeunit_to_project_version(file,version)
    replace_version_in_nuspec_file(sc,GeneralUtilities.resolve_relative_path("./Build/GRYLibrary.nuspec",folder_of_current_file),version)
    replace_version_in_csproj_file(sc,GeneralUtilities.resolve_relative_path("./GRYLibrary/GRYLibrary.csproj",folder_of_current_file),version)

common_tasks()
