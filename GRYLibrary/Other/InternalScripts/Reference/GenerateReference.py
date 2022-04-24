import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities

def standardized_tasks_generate_refefrence_for_dotnet_project_in_common_project_structure(self:ScriptCollectionCore,repository:str):
    reference_folder=os.path.join(repository,"GRYLibrary","Other","InternalScripts","Reference")
    reference_result_folder=os.path.join(reference_folder,"Result")
    GeneralUtilities.ensure_directory_does_not_exist(reference_result_folder)
    self.run_program("docfx","docfx.json",reference_folder)

def generate_reference():
    standardized_tasks_generate_refefrence_for_dotnet_project_in_common_project_structure(ScriptCollectionCore(),str(Path(os.path.dirname(__file__)).parent.absolute()), "GRYLibraryTests", "QualityCheck")

if __name__ == "__main__":
    generate_reference()
