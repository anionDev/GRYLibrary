import os
import sys
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities


def generate_reference():
    ScriptCollectionCore().standardized_tasks_generate_refefrence_for_dotnet_project_in_common_project_structure(str(Path(__file__).absolute()), sys.argv)

generate_reference()
