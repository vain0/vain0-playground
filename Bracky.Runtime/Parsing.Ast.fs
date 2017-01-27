﻿namespace Bracky.Runtime.Parsing

open FParsec

type VariableOccurrence =
  {
    Name:
      string
    Id:
      int64
  }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module VariableOccurrence =
  let count = ref 0L

  let create name =
    let identifier = !count
    count := identifier + 1L
    {
      Name = name
      Id = identifier
    }

  let positionFree name =
    {
      Name = name
      Id = -1L
    }

type Pattern =
  | VariablePattern
    of Position * VariableOccurrence
with
  member this.Position =
    match this with
    | VariablePattern (position, _) ->
      position

  member this.SetPosition(position) =
    match this with
    | VariablePattern (_, occurence) ->
      VariablePattern (position, VariableOccurrence.positionFree occurence.Name)

type BinaryOperator =
  | ApplyOperator
  | AddOperator
  | MulOperator
  | ThenOperator

type Expression =
  | IntExpression
    of Position * int64
  | BoolExpression
    of Position * bool
  /// Represents an expression to read the value of variable.
  | RefExpression
    of Position * string
  | FunExpression
    of Position * Pattern * Expression
  | IfExpression
    of IfClause * array<IfClause>
  | BinaryOperationExpression
    of BinaryOperator * Expression * Expression
  | ValExpression
    of Pattern * Expression
with
  member this.Position =
    match this with
    | IntExpression (position, _) ->
      position
    | BoolExpression (position, _) ->
      position
    | RefExpression (position, _) ->
      position
    | FunExpression (position, _, _) ->
      position
    | IfExpression (clause, _) ->
      clause.Position
    | BinaryOperationExpression (_, left, _) ->
      left.Position
    | ValExpression (pattern, _) ->
      pattern.Position
      
  member this.SetPosition(position) =
    match this with
    | IntExpression (_, value) ->
      IntExpression (position, value)
    | BoolExpression (_, value) ->
      BoolExpression (position, value)
    | RefExpression (_, name) ->
      RefExpression (position, name)
    | FunExpression (_, pattern, expression) ->
      let expression = expression.SetPosition(position)
      FunExpression (position, pattern.SetPosition(position), expression)
    | IfExpression (head, tail) ->
      let tail = tail |> Array.map (fun c -> c.SetPosition(position))
      IfExpression (head.SetPosition(position), tail)
    | BinaryOperationExpression (operator, left, right) ->
      let left = left.SetPosition(position)
      let right = right.SetPosition(position)
      BinaryOperationExpression (operator, left, right)
    | ValExpression (pattern, expression) ->
      ValExpression (pattern.SetPosition(position), expression.SetPosition(position))

and IfClause =
  | IfClause
    of Expression * Expression
  | ElseClause
    of Expression
with
  member this.Position =
    match this with
    | IfClause (condition, _) ->
      condition.Position
    | ElseClause expression ->
      expression.Position

  member this.SetPosition(position) =
    match this with
    | IfClause (condition, expression) ->
      IfClause (condition.SetPosition(position), expression.SetPosition(position))
    | ElseClause expression ->
      ElseClause (expression.SetPosition(position))
