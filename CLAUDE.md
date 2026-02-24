# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

Monorepo with a single `frontend/` directory containing the full application. All commands should be run from `frontend/`.

## Quick Reference

```bash
cd frontend
yarn dev       # Start dev server
yarn build     # Production build (does NOT type-check; use `npx vue-tsc -b` separately)
yarn preview   # Preview production build
```

See `frontend/CLAUDE.md` for detailed architecture, conventions, and tier system documentation.
