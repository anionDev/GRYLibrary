import sys
from ScriptCollection.TFCPS_CodeUnitSpecific_DotNet import TFCPS_CodeUnitSpecific_DotNet_Functions,TFCPS_CodeUnitSpecific_DotNet_CLI
 

def update_dependencies():
    tf:TFCPS_CodeUnitSpecific_DotNet_Functions=TFCPS_CodeUnitSpecific_DotNet_CLI.parse(__file__,sys.argv)
    tf.update_dependencies()


if __name__ == "__main__":
    update_dependencies()
