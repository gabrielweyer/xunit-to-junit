# Developing #

## Building locally ##

Run this command to build on Windows:

```powershell
.\build.ps1
```

Run this command to build on Linux / macOS:

```shell
./build.sh
```

Add `--package` to also create the NuGet package in `artifacts/packages`.

## Releasing ##

### How versioning works ###

- The version is computed by [GitVersion][gitversion] (configured in `GitVersion.yml`), from the most recent version tag reachable from `HEAD`.
- Every push to `main` builds and **releases**: the workflow creates a GitHub release named `v<version>` and pushes the package to NuGet.
- Without a tag on `HEAD`, the version is the latest tag with its patch number incremented (e.g. `v8.0.0` → `8.0.1`).
- No release is created when the `HEAD` commit message contains `[skip-release]` or `dependabot[bot]`.
- Changes that only touch `README.md`, `DEVELOPING.md`, `LICENSE`, `.editorconfig`, `.vscode/`, `.github/dependabot.yml` or the other workflows don't run the build at all, so they don't release.

### Patch release ###

Nothing to do: merge to `main` and a patch release is published.

### Minor or major release ###

Tags use the `v` prefix: `v8.0.0`, **not** `8.0.0`.

The tag must be on the exact commit that ends up at the tip of `main`. GitHub's merge button breaks this (squash and rebase rewrite commits, a merge commit adds one on top), so merge locally instead:

1. Get the Pull Request green.
2. Tag the last commit of the branch with an annotated tag:

   ```shell
   git tag -a v8.0.0 -m "v8.0.0 <summary of the breaking change>"
   ```

3. Fast-forward `main` to the branch:

   ```shell
   git switch main
   git merge --ff-only <branch>
   ```

4. Push `main` and the tag together, so that the build sees the tag:

   ```shell
   git push --atomic origin main v8.0.0
   ```

GitHub closes the Pull Request as merged. The release workflow reuses the existing tag for the GitHub release.

If you add a commit after tagging, move the tag to the new last commit (`git tag -a -f v8.0.0 -m "..."`) before pushing, otherwise the release is `8.0.1`.

### Fixing a wrong tag ###

If you created the wrong tag (e.g. `8.0.0` instead of `v8.0.0`) and haven't pushed `main` yet:

```shell
git tag -a v8.0.0 -m "v8.0.0 <summary>" 8.0.0^{}
git tag -d 8.0.0
git push origin :refs/tags/8.0.0
git push --atomic origin main v8.0.0
```

Pushing a tag alone doesn't trigger a build, so a wrong tag pushed on its own is harmless until `main` is pushed.

### Prereleases ###

A Pull Request from a `features/<name>` branch publishes a GitHub prerelease (not pushed to NuGet), unless the `HEAD` commit message contains `[skip-release]`.

[gitversion]: https://gitversion.net/docs/
