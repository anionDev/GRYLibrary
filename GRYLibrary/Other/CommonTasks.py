import sys
import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure


def common_tasks():
    file = str(Path(__file__).absolute())
    cmd_args = sys.argv
    sc = ScriptCollectionCore()
    t = TasksForCommonProjectStructure()
    verbosity = t.get_verbosity_from_commandline_arguments(cmd_args, 1)
    codeunitname = os.path.basename(GeneralUtilities.resolve_relative_path("..", os.path.dirname(file)))
    build_environment = t.get_targetenvironmenttype_from_commandline_arguments(cmd_args, "QualityCheck")
    additional_arguments_file = t.get_additionalargumentsfile_from_commandline_arguments(cmd_args, None)
    folder_of_current_file = os.path.dirname(file)
    version = sc.get_semver_version_from_gitversion(GeneralUtilities.resolve_relative_path("../..", os.path.dirname(file)))  # Should always be the same as the project-version
    sc.replace_version_in_nuspec_file(GeneralUtilities.resolve_relative_path(f"./Build/{codeunitname}.nuspec", folder_of_current_file), version)
    sc.replace_version_in_csproj_file(GeneralUtilities.resolve_relative_path(f"../{codeunitname}/{codeunitname}.csproj", folder_of_current_file), version)
    sc.replace_version_in_csproj_file(GeneralUtilities.resolve_relative_path(f"../{codeunitname}Tests/{codeunitname}Tests.csproj", folder_of_current_file), version)
    t.standardized_tasks_do_common_tasks(file, version, verbosity, build_environment, True, additional_arguments_file, cmd_args)


if __name__ == "__main__":
    common_tasks()
