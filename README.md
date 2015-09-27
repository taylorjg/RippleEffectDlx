
## Description

I was [recently contacted ](https://github.com/taylorjg/DlxLib/pull/2)
by somebody with reference to (the lack of) support for secondary columns in
[my implementation](https://github.com/taylorjg/DlxLib)
of [DLX](https://en.wikipedia.org/wiki/Dancing_Links). I must confess that I did
not make it to the end of the [original paper](http://arxiv.org/pdf/cs/0011047v1.pdf).
I am now in the process of adding this support to
[DlxLib](https://github.com/taylorjg/DlxLib). As such, I thought I would try to solve
[Ripple Effect](https://en.wikipedia.org/wiki/Ripple_Effect_(puzzle)) puzzles because
this is what motivated the person mentioned above.
It took me a while to figure out how to do this but I finally got there.

I also have another project which makes use of secondary columns - see
[TetraSticks](https://github.com/taylorjg/TetraSticks).

## Details of the DLX Matrix

### Primary columns

* One primary column per position (row/col) in the grid.
These primary columns ensure that the grid is fully populated
i.e. every position in the grid is filled exactly once.

* One primary column for each value in each room.
These primary columns ensure that every room is fully populated
and that there are no duplicate values in a room. For example,
a room with four cells must contain one each of 1, 2, 3 and 4.

### Secondary columns

There are four sets of secondary columns for each unique number
that appears in the grid. For example, if the grid consists of a mixture
of rooms with 1, 2, 3, 4 and 5 cells, then the solved puzzle will
contain a mixture of 1s, 2s, 3s, 4s and 5s. In this case, we will
have 4 x 5 = 20 sets of secondary columns. The number of columns
in each set of secondary columns will be numRows x numCols. These sets
of secondary columns are used to enforce the proximity constraints.
This is best explained using an example. Say we have a 5 x 5 grid with
position (0, 0) at bottom left. Lets say that we have a 2 at (1, 3):

```
-----
-2---
-----
-----
-----
```

The first set of secondary columns represents the position of the 2 itself plus
the positions above that must not contain another 2 (one of these lies off the grid):

```
-x---   (01000)
-x---   (01000)
-----   (00000)
-----   (00000)
-----   (00000)

0000000000000000100001000
```

The second set of secondary columns represents the position of the 2 itself plus
the positions below that must not contain another 2:

```
-----   (00000)
-x---   (01000)
-x---   (01000)
-x---   (01000)
-----   (00000)

0000001000010000100000000
```

The third set of secondary columns represents the position of the 2 itself plus
the positions to the left that must not contain another 2 (one of these lies off the grid):

```
-----   (00000)
xx---   (11000)
-----   (00000)
-----   (00000)
-----   (00000)

0000000000000001100000000
```

The fourth set of secondary columns represents the position of the 2 itself plus
the positions to the right that must not contain another 2:

```
-----   (00000)
-xxx-   (01110)
-----   (00000)
-----   (00000)
-----   (00000)

0000000000000000111000000
```

I refer to these up/down/left/right values as the 'ripples' caused by
placing a value. Maybe this is why they are called Ripple Effect puzzles ?

Of the 20 sets of secondary columns, only 4 of them will be populated in any particular row.
The other 16 sets of secondary columns will be filled
with 0s. So in other words, we have a set of secondary columns for each of the following:

* the up/down/left/right ripples of placed 1s
* the up/down/left/right ripples of placed 2s
* the up/down/left/right ripples of placed 3s
* the up/down/left/right ripples of placed 4s
* the up/down/left/right ripples of placed 5s

## Screenshot

There is a [Ripple Effect Tutorial](http://www.nikoli.co.jp/en/puzzles/ripple_effect.html).
This is a screen grab of the puzzle used in the tutorial:

![RippleEffectTutorial](https://raw.github.com/taylorjg/RippleEffectDlx/master/Images/RippleEffectTutorial.png)

This is a screenshot of RippleEffectDlxConsole solving the above puzzle:

![Screenshot](https://raw.github.com/taylorjg/RippleEffectDlx/master/Images/Screenshot.png)

## Future Plans

I now have a quick and dirty console app that proves that this type of puzzle
can be solved using [DLX](https://en.wikipedia.org/wiki/Dancing_Links) with secondary columns.
My next goal is to write a WPF app that visualises each step of the algorithm as it solves the puzzle.

## WPF App TODO List

* ~~Get basic window working with background and grid~~
* ~~Draw the cells of a room~~
* ~~Draw the border around a room~~
* ~~Draw a number in a cell~~
* ~~Draw an initial value in a cell (like drawing a number but needs to look slightly different)~~
* ~~Solve the puzzle on a different thread~~
* ~~Render the solution~~
* ~~Render the search steps~~
* Add a Solve button
* Add a Cancel button
* Add a slider to control the speed at which the search steps are displayed

## Links

* https://en.wikipedia.org/wiki/Ripple_Effect_(puzzle)
* http://www.nikoli.co.jp/en/puzzles/ripple_effect.html
* http://www.nikoli.com/en/puzzles/ripple_effect/
* [DLX](https://en.wikipedia.org/wiki/Dancing_Links)
* [Algorithm X](https://en.wikipedia.org/wiki/Knuth%27s_Algorithm_X)
* [DlxLib](https://github.com/taylorjg/DlxLib)
