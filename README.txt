
EcoCov: Economic Analysis for Problem Resolution in Structural Test Generation
==========================================================================

Project website:
https://sites.google.com/site/asergrp/projects/ecocov


* Instruction on how to use EcoCov

1. Install VS2010 and Pex.
2. Add EcoCov project into your project reference.
3. Add the following 7 properties in the AssemblyInfo.cs of your test project:
[assembly: IssueTrack]
[assembly: ProblemObserver]
[assembly: IssueObserver]
[assembly: AssemblyCoverageObserver]
[assembly: ResultTrackingObserver]
[assembly: PUTObserver]
[assembly: PUTCoverageObserver]
4. Right click the method or class under test and select "Create Parameterized Unit Tests".
5. Right click the generated PUT and then select "Run Pex Explorations".
6. Set EcoCov project as the startup project and Run EcoCov (sample usage in the code comments). 


This is prototype implemented for our EcoCov framework and is released under the following license:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Please contact Sihan Li (sihanli2@illinois.edu) for any doubts or suggestions.