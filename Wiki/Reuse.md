# The Holy Grail of Reuse
Whether you’re comparing software development to the industrial revolution or the hardware industry, there’s clearly a lot of room for improvement.  Pick your preferred metaphor, and picture building every piece of software by taking an established pattern, harvesting the vast majority of architectural decisions and functionality without delving into source code, and building only that which differentiates your project from all that gone before.

Reuse as a compelling concept in software is hardly new, in fact it has been touted as the cure for software ills for so long that it’s something of a cliché.  Worried about quality?  Reuse existing code that has withstood the test of time.  Looking to reduce time-to-market?  Reuse as much as you can and dramatically improve the productivity of your team.  Faced with rampant complexity?  Reuse a well-known solution to eliminate the learning curve for part of your project.

Despite the truth of these statements, the only pervasive example of reuse you can count on from one software project to the next is that they’ll make extensive use of a single underlying platform.  Win32 and .NET are two of the most widely reused bodies of code in existence.

So what prevents people from reusing code more broadly?

# Exploring Reuse Blockers
There is no single reason why code doesn’t get reused more often.  Instead there are numerous factors which complicate the situation, each of which deserves individual attention.

### Complexity
Successful projects tend to start off simple and grow in complexity gradually.  In the long run a general solution that may well be the ideal solution, but being forced to cope with the full weight at the beginning of a project will cause immediate resistance.  It’s critical that the complexity be introduced gradually to keep the perceived complexity in line with the realized value.
Great documentation, good samples, and easy integration into the software tool-chain are necessary but not sufficient.  The ability to incrementally adopt a subset of the full solution and get “easy wins” at each stage of adoption is critical.

### Dependencies
When you choose to use existing code, one of the first issues you’ll be faced with is the list of dependencies it carries.  There are two ideal scenarios: adopting a solution that is entirely self-contained, or a solution that happens to carry dependencies you have already taken for your own code.  This issue plays a large part in the relative success of broad platforms over individual libraries.  A platform represents a single dependency decision with presumed interoperability among all of its constituent parts.
 
Limiting dependencies is critical to the success of code that is designed for reuse.  Two techniques that can help are using abstract rather than concrete dependencies (any implementation that satisfies a given contract will suffice rather than requiring a particular implementation), and making dependencies optional (presumably reducing the functionality of the reused code when the dependency is missing.)

### Rigidity
Pre-existing solutions that offer a fixed set of functionality will be applicable in fewer situations than a more flexible alternative.  Inflexibility may arise from functionality that is incomplete, extraneous, or created to meet a different set of design trade-offs.
Compartmentalizing a solution so that it can be adopted piecemeal is one approach to addressing this problem.  The parts that fit can be adopted, presumably reducing the total amount of code that needs to be written for a given project.  Keeping the solution open-ended is equally important and easier than anticipating the exact needs of every possible usage scenario.

### Deployment
Adopting someone else’s technology confers responsibility for deploying it along with your own work; this is another reason why sticking with an established platform is a common pattern.  The ideal solution entails friction-free deployment, unrestrictive licensing, and zero configuration or administration.  Each deviation from this ideal reduces the likelihood that a solution will be reused.  This problem doesn’t disappear in a hosted world.  The challenges of deploying code to a hosting environment are analogous, and while they are frequently amortized across many more end users the initial barrier to a “quick win” can still be a reuse blocker.
