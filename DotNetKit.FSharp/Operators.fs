﻿namespace DotNetKit.FSharp

open System

[<AutoOpen>]
module Operators =
  let tap f x =
    f x
    x

  let flip f x y =
    f y x

  let todo message =
    NotImplementedException(message) |> raise

  let tryCast<'x, 'y> (x: 'x) =
    match x |> box with
    | :? 'y as y ->
      Some y
    | _ ->
      None

  let fold (xs: #seq<'x>) (f: 'x -> 's -> 's) (s: 's): 's =
    xs |> Seq.fold (flip f) s 
