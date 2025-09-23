import sys
from ScriptCollection.TFCPS_CodeUnitSpecific_DotNet import TFCPS_CodeUnitSpecific_DotNet_Functions,TFCPS_CodeUnitSpecific_DotNet_CLI
  
def generate_reference():
    
    tf:TFCPS_CodeUnitSpecific_DotNet_Functions=TFCPS_CodeUnitSpecific_DotNet_CLI.parse(__file__,sys.argv)
    tf.generate_reference()


if __name__ == "__main__":
    generate_reference()
