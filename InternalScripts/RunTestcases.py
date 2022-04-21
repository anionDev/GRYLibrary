import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore

def run_testcases_for_csharp_project(sc:ScriptCollectionCore,repository_folder:str):
    sc.start_program_synchronously("dotnet", f"clean -c QualityCheck" , repository_folder, prevent_using_epew=True)
    sc.start_program_synchronously("dotnet", f"build GRYLibraryTests/GRYLibraryTests.csproj -c QualityCheck" , repository_folder, prevent_using_epew=True)
    sc.start_program_synchronously("dotnet", f"test GRYLibraryTests/GRYLibraryTests.csproj -c QualityCheck" \
        f" --verbosity normal /p:CollectCoverage=true /p:CoverletOutput=/Other/TestCoverage/TestCoverage.xml" \
        f" /p:CoverletOutputFormat=opencover", repository_folder, prevent_using_epew=True)

def RunTestcasesStarter():
    sc=ScriptCollectionCore()
    repository_folder=str(Path(os.path.dirname(__file__)).parent.absolute())
    run_testcases_for_csharp_project(sc,repository_folder)
    sc.generate_coverage_report(repository_folder)

if __name__=="__main__":
    RunTestcasesStarter()
