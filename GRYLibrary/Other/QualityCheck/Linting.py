import sys
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore


def linting():
    ScriptCollectionCore().standardized_tasks_linting_for_dotnet_project_in_common_project_structure(str(Path(__file__).absolute()), sys.argv)


if __name__ == "__main__":
    linting()
