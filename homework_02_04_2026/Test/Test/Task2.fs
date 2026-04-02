// <copyright file="Task2.fs" company="Bengya Kirill">
// Copyright (c) Bengya Kirill under MIT License.
// </copyright>

module Test.Task2

/// <summary>
/// Generates a string representation of a square with the given side size,
/// where the border is made of '*' and the interior is filled with spaces.
/// </summary>
/// <param name="sideSize">The length of the side of the square.</param>
/// <returns>A string representing the square.</returns>
let getSquare sideSize =
    let rec getHorizontalSide item index acc =
        if index <= 0 then
            acc
        else
            getHorizontalSide item (index - 1) (acc + string item)

    let rec getVerticalSide index acc =
        if index <= 0 then
            acc
        else
            getVerticalSide (index - 1) (acc + "\n*" + getHorizontalSide ' ' (sideSize - 2) "" + "*")

    let result =
        getHorizontalSide '*' sideSize ""
        + getVerticalSide (sideSize - 2) ""
        + "\n"
        + if sideSize <= 1 then
              ""
          else
              getHorizontalSide '*' sideSize ""

    result
