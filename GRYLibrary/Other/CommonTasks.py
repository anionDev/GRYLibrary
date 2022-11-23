import sys
import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure


def common_tasks():
    file = str(Path(__file__).absolute())
    folder_of_current_file = os.path.dirname(file)
    cmd_args = sys.argv
    codeunit_folder = GeneralUtilities.resolve_relative_path("..", os.path.dirname(file))
    sc = ScriptCollectionCore()
    t = TasksForCommonProjectStructure()
    verbosity = t.get_verbosity_from_commandline_arguments(cmd_args, 1)
    build_environment = t.get_targetenvironmenttype_from_commandline_arguments(cmd_args, "QualityCheck")
    additional_arguments_file = t.get_additionalargumentsfile_from_commandline_arguments(cmd_args, None)
    version = sc.get_semver_version_from_gitversion(GeneralUtilities.resolve_relative_path("../..", os.path.dirname(file)))
    t.standardized_tasks_do_common_tasks(file, version, verbosity, build_environment, True, additional_arguments_file, cmd_args)
    t.replace_common_variables_in_nuspec_file(codeunit_folder)
    sc.replace_version_in_csproj_file(GeneralUtilities.resolve_relative_path("../GRYLibrary/GRYLibrary.csproj", folder_of_current_file), version)
    sc.replace_version_in_csproj_file(GeneralUtilities.resolve_relative_path("../GRYLibraryTests/GRYLibraryTests.csproj", folder_of_current_file), version)


if __name__ == "__main__":
    common_tasks()
