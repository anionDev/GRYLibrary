from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore


def generate_reference():
    ScriptCollectionCore().standardized_tasks_generate_reference_by_docfx(Path(__file__).absolute())


generate_reference()
