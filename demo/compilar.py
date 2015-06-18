from pathlib import Path
from shutil import copy2, copytree, ignore_patterns
import argparse
import os
import subprocess
from tempfile import TemporaryDirectory

root = Path(__file__).resolve().parents[1]
parser = argparse.ArgumentParser(description="Build the inventory site / Compila el sitio de inventarios")
parser.add_argument("--output", type=Path, required=True)
options = parser.parse_args()
target = options.output.resolve()
if target.exists() or target.is_relative_to(root):
    parser.error("Use a new output directory outside the repository / Usa un directorio nuevo fuera del repositorio")

with TemporaryDirectory(prefix="inventory-web-build-") as scratch:
    build = Path(scratch)
    site = build / "inventarios"
    copytree(root / "inventarios", site, ignore=ignore_patterns("Web.config", "obj", "App_Data"))
    copy2(root / "demo/Web.config.example", site / "Web.config")
    library = build / "src/SDG.Componentes"
    copytree(root / "src/SDG.Componentes", library, ignore=ignore_patterns("bin", "obj"))
    project = library / "SDG.Componentes.csproj"
    subprocess.run(["dotnet", "build", str(project), "-c", "Release", "--nologo"], check=True)
    copy2(library / "bin/Release/net48/SDG.Componentes.dll", site / "Bin")
    compiler = Path(os.environ["SystemRoot"]) / "Microsoft.NET/Framework64/v4.0.30319/aspnet_compiler.exe"
    subprocess.run([str(compiler), "-v", "/inventarios", "-p", str(site), "-f", str(target)], check=True)
