## DiffLib

DiffLib is a library of diffing functionality in C#; similar to the functionality provided by the Python diffing tools.

### Getting started

**Getting started with Git and GitHub**

 * People new to GitHub should consider using [GitHub for Windows](http://windows.github.com/).
 * If you decide not to use GHFW you will need to:
  1. [Set up Git and connect to GitHub](http://help.github.com/win-set-up-git/)
  2. [Fork the DiffLib repository](http://help.github.com/fork-a-repo/)
 * Finally you should look into [git - the simple guide](http://rogerdudler.github.com/git-guide/)

**Rules for Our Git Repository**

 * We use ["A successful Git branching model"](http://nvie.com/posts/a-successful-git-branching-model/). What this means is that:
   * You need to branch off of the [develop branch](https://github.com/jcdickinson/DiffLib) when creating new features or non-critical bug fixes.
   * Each logical unit of work must come from a single and unique branch:
     * A logical unit of work could be a set of related bugs or a feature.
     * You should wait for us to accept the pull request (or you can cancel it) before committing to that branch again.
     
### License

DiffLib uses the BSD 3-clause license, which can be found in license.txt.

**Additional Restrictions**

 * We only accept code that is compatible with the BSD license (essentially, MIT and Public Domain).
 * Copying copy-left (GPL-style) code is strictly forbidden.

### LCS Algorithms

* [Patience Diff](http://alfedenzo.livejournal.com/170301.html)

### Stability

* DiffLib lacks a good amount of unit tests.