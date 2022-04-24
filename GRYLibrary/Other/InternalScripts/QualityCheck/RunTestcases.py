import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities

def run_testcases():
    ScriptCollectionCore().standardized_tasks_run_testcases_for_dotnet_project_in_common_project_structure(str(Path(os.path.dirname(__file__)).parent.absolute()), "GRYLibraryTests", "QualityCheck")

if __name__ == "__main__":
    run_testcases()
