import sys
from pathlib import Path
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure


def build():
    t = TasksForCommonProjectStructure()
    t.standardized_tasks_build_for_dotnet_library_project_in_common_project_structure(
        str(Path(__file__).absolute()), "QualityCheck", t.get_default_target_environmenttype_mapping(), ["win", "linux"], 1, sys.argv)


if __name__ == "__main__":
    build()
