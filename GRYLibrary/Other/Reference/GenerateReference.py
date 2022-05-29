import os
import sys
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore


def standardized_tasks_generate_refefrence_for_dotnet_project_in_common_project_structure(self: ScriptCollectionCore, generate_reference_file:str, commandline_arguments: list[str] = []):
    reference_folder=os.path.dirname(generate_reference_file)
    reference_result_folder = os.path.join(reference_folder, "GeneratedReference")
    GeneralUtilities.ensure_directory_does_not_exist(reference_result_folder)
    self.run_program("docfx", "docfx.json", reference_folder)


def generate_reference():
    standardized_tasks_generate_refefrence_for_dotnet_project_in_common_project_structure(ScriptCollectionCore(), str(Path(__file__).absolute()), sys.argv)

generate_reference()
