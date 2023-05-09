# Language Design

My language has two target groups:
* Story writers with little to no programming experience, they should be able to express their story without having to spend a long time to learn the syntax.
* Programmers who implement the story in cooperation with a writer. They should be able to express the intent of the writer without issue.

For the scope of this research I focus on the story writers.

When determining the language structure, there are some parts that do not require much research to my target group, as these parts are universal, such as arithmetics, they have a set way of interacting.

## Research

For the scope of this research I focussed on 3 parts that together form the fiber of my language: Choices, Jumps and Variables.

I also want to find out what tool(s) they currently use for writing their stories and how experienced they are with programming.

In the questionnaire I used a mixture of open and closed questions, In order to determine preferences for syntax styles I gave three examples where they can rank them to their preferences.

After handing out the questionnaire to my target group I found out that:
* Twine is a tool that is used often in my target group.
* The range of programming skills varies.
* There was no clear favorite for choice syntax. When asked for why they chose their favorites it was clear however that being able to separate the choice from the response was more desirable due to being easier to read.
* It should be possible to hide or show the choice the reader has to make in a story, However there was no common consensus if it should be included by default or not.
* After the story jumps to a different part, the expected behavior is that it runs to completion.
* Jumps should be able to be given arguments that can be used to make it more dynamic.
* Ink style jumps (`->target` for making the jump, `== target ==` for where the jump leads to) seem to be more favorable on average due to it's ease of understanding given as a common reason.
* It is quite unanimous that type safety should be enforced.
* Pascal style type declaration (`variable : type`) seems to be the favorite.


Due to the small scope of the research I will use the data I got for the current prototype, but will be open to changes with a more in depth research.

### Future

The language is far from being truly done, I plan on evolving it over time to add more features and interactions.

I would like to be able to add:
* Calling functions outside of the language's runtime. This can be useful for doing complex math or getting outside variables.
* Variable states, this would be a syntactic sugar to make determining a state more readable and approachable, this would be like C#'s `enum`.
* Variable flags, this would make defining various story conditions easier.
* and more...
