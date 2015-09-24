
## Description

I was [recently contacted ](https://github.com/taylorjg/DlxLib/pull/2)
by somebody with reference to (the lack of) support for secondary columns in [my implementation](https://github.com/taylorjg/DlxLib)
of [DLX](https://en.wikipedia.org/wiki/Dancing_Links). I must confess that I did not make it to the end of the [original paper](http://arxiv.org/pdf/cs/0011047v1.pdf). I am now in the process of adding this support to DlxLib. As such, I thought I would try to solve [Ripple Effect](https://en.wikipedia.org/wiki/Ripple_Effect_(puzzle)) puzzles because this is what motivated the person mentioned above. It took me a while to figure out how to do this but I finally got there.

I also have another project which makes use of secondary columns - see [TetraSticks](https://github.com/taylorjg/TetraSticks).

## Details of the DLX Matrix

_TODO_

## Screenshot

Here is an example Ripple Effect puzzle:

![Example Ripple Puzzle](http://www.sachsentext.de/gif/ripple_effect1.gif)

Here is a screenshot showing my RippleEffectDlxConsole program solving the above puzzle:

![Screenshot](https://raw.github.com/taylorjg/RippleEffectDlx/master/Images/Screenshot.png)

## Future Plans

I have a quick and dirty console app that proves that this type of puzzle
can be solved using DLX with secondary columns. My next goal is to write a WPF app that visualises each step of the algorithm as it solves the puzzle.

## Links

* https://en.wikipedia.org/wiki/Ripple_Effect_(puzzle)
* http://www.nikoli.co.jp/en/puzzles/ripple_effect.html
* http://www.nikoli.com/en/puzzles/ripple_effect/
* [DLX](https://en.wikipedia.org/wiki/Dancing_Links)
* [Algorithm X](https://en.wikipedia.org/wiki/Knuth%27s_Algorithm_X)
* [DlxLib](https://github.com/taylorjg/DlxLib)
