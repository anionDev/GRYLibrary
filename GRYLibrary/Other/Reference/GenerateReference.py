import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities


def standardized_tasks_generate_refefrence_for_dotnet_project_in_common_project_structure(self: ScriptCollectionCore, generate_reference_file:str):
    reference_folder=os.path.dirname(generate_reference_file)
    reference_result_folder = os.path.join(reference_folder, "GeneratedReference")
    GeneralUtilities.ensure_directory_does_not_exist(reference_result_folder)
    self.run_program("docfx", "docfx.json", reference_folder)


def generate_reference():
    standardized_tasks_generate_refefrence_for_dotnet_project_in_common_project_structure(ScriptCollectionCore(), str(Path(__file__).absolute()))


if __name__ == "__main__":
    generate_reference()
