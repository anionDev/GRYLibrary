import sys
from ScriptCollection.TFCPS_CodeUnitSpecific_DotNet import TFCPS_CodeUnitSpecific_DotNet_Functions,TFCPS_CodeUnitSpecific_DotNet_CLI

 
def common_tasks():
    tf:TFCPS_CodeUnitSpecific_DotNet_Functions=TFCPS_CodeUnitSpecific_DotNet_CLI.parse(__file__,sys.argv)
    tf.do_common_tasks(tf.get_version_of_project())#codeunit-version should alsways be the same as project-version


if __name__ == "__main__":
    common_tasks()
