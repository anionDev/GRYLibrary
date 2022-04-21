import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities


def run_testcases():
    sc=ScriptCollectionCore()
    repository_folder=str(Path(os.path.dirname(__file__)).parent.absolute())
    sc.run_testcases_for_csharp_project(sc,repository_folder,"QualityCheck")
    sc.generate_coverage_report(repository_folder)

if __name__=="__main__":
    run_testcases()
