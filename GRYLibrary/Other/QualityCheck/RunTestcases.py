import os
import sys
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore

def run_testcases():
    ScriptCollectionCore().standardized_tasks_run_testcases_for_dotnet_project_in_common_project_structure(str(Path(__file__).absolute()), "QualityCheck", sys.argv)


if __name__ == "__main__":
    run_testcases()
