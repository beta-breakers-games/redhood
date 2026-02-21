# Cloning This Unity Project on Windows (with Git LFS)

Repository: https://github.com/beta-breakers-games/redhood/

---

## 1. Install Git for Windows

Download: https://git-scm.com/download/win

Verify installation (Git Bash):

```bash
git --version
```

## 2. Install Git LFS

Download:
https://git-lfs.com/

Initialize (Git Bash):
```bash
git lfs install
git lfs --version
```
This step is required once per machine.

## 3. Clone the Repository
```bash
git clone https://github.com/beta-breakers-games/redhood.git
cd redhood
```

## 4. Ensure LFS Files Are Downloaded

```bash
git lfs pull
git lfs ls-files
```
If git lfs ls-files lists files, LFS is working correctly.

## 5. Open in Unity

Install the correct Unity 6 version via Unity Hub.

Open Unity Hub.

Click Add project.

Select the `redhood` folder.

Unity will regenerate the Library/ folder automatically.

**Troubleshooting**
LFS files appear as small text files

```bash
git lfs pull
git lfs install
```
Authentication fails

If the repository is private:

Use a GitHub Personal Access Token (HTTPS)
or
Configure SSH keys for GitHub
