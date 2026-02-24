#!/usr/bin/env python3

from argparse import ArgumentParser
import json
import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.abspath(__file__))

parser = ArgumentParser(description="Datacenter DCIM manager")
parser.add_argument("action", choices=["run", "build"], help="Action: run | build")
parser.add_argument("type", choices=["backend", "frontend", "app"], help="Target: backend | frontend | app")
parser.add_argument("deploy", help="Environment: dev | prod")

args = parser.parse_args()

# ── Load config ──
config_path = os.path.join(ROOT, "config.json")
with open(config_path) as f:
    config = json.load(f)

if args.deploy not in config:
    print(f"Environment '{args.deploy}' not found in config.json. Available: {', '.join(config.keys())}")
    sys.exit(1)

env = config[args.deploy]
print(f"[manage] environment: {args.deploy}")
for k, v in env.items():
    print(f"  {k} = {v}")

# ── Generate frontend .env from template ──
def write_frontend_env():
    template_path = os.path.join(ROOT, "frontend.template.env")
    with open(template_path) as f:
        content = f.read()
    for key, val in env.items():
        content = content.replace(f"#{{{key}}}", val)
    env_path = os.path.join(ROOT, "frontend", ".env")
    with open(env_path, "w") as f:
        f.write(content)
    print(f"[manage] wrote frontend/.env")

# ── Copy frontend dist into backend/wwwroot ──
def copy_frontend_to_wwwroot():
    dist = os.path.join(ROOT, "frontend", "dist")
    wwwroot = os.path.join(ROOT, "backend", "wwwroot")
    if os.path.exists(wwwroot):
        shutil.rmtree(wwwroot)
    shutil.copytree(dist, wwwroot)
    print(f"[manage] copied frontend/dist → backend/wwwroot")

# ── Run steps ──
def run(cmd, cwd=ROOT):
    print(f"\n[manage] $ {cmd}")
    print(f"[manage]   cwd: {cwd}")
    result = subprocess.run(cmd, shell=True, cwd=cwd)
    if result.returncode != 0:
        print(f"[manage] command failed with exit code {result.returncode}")
        sys.exit(result.returncode)

steps = []

if args.type == "frontend":
    write_frontend_env()
    if args.action == "run":
        steps = [("yarn dev", os.path.join(ROOT, "frontend"))]
    elif args.action == "build":
        steps = [("yarn build", os.path.join(ROOT, "frontend"))]

elif args.type == "backend":
    write_frontend_env()
    if args.action == "run":
        steps = [("dotnet run", os.path.join(ROOT, "backend"))]
    elif args.action == "build":
        steps = [("dotnet build -c Release", os.path.join(ROOT, "backend"))]

elif args.type == "app":
    write_frontend_env()
    if args.action == "run":
        steps = [
            ("dotnet build -c Release", os.path.join(ROOT, "backend")),
            ("yarn build", os.path.join(ROOT, "frontend")),
        ]
        # copy_frontend_to_wwwroot runs between build and serve
    elif args.action == "build":
        steps = [
            ("dotnet build -c Release", os.path.join(ROOT, "backend")),
            ("yarn build", os.path.join(ROOT, "frontend")),
        ]

# Execute build steps
for cmd, cwd in steps:
    run(cmd, cwd)

# For app mode, copy dist to wwwroot then run/report
if args.type == "app":
    copy_frontend_to_wwwroot()
    if args.action == "run":
        run("dotnet run -c Release", os.path.join(ROOT, "backend"))
    elif args.action == "build":
        print("\n[manage] app build complete. Run with: cd backend && dotnet run -c Release")
