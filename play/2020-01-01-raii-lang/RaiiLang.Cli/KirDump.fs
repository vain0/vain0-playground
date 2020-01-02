module rec RaiiLang.KirDump

open RaiiLang.Helpers
open RaiiLang.Kir

let callByToString callBy =
  match callBy with
  | ByMove ->
    "move"

  | ByRef ->
    "ref"

let kdParam param _indent acc =
  match param with
  | KParam (callBy, name) ->
    acc
    |> cons (callByToString callBy)
    |> cons " "
    |> cons name

let kdParamList paramList indent acc =
  let acc = acc |> cons "("

  let _, acc =
    paramList
    |> List.fold (fun (sep, acc) param ->
      let acc =
        acc
        |> cons sep
        |> kdParam param indent
      ", ", acc
    ) ("", acc)

  acc |> cons ")"

let kdArg arg indent acc =
  match arg with
  | KArg (callBy, node) ->
    acc
    |> cons (callByToString callBy)
    |> cons " "
    |> kdNode node indent

let kdArgList args indent acc =
  let acc = acc |> cons "("

  let acc =
    match args with
    | [] ->
      acc

    | [arg] ->
      acc |> kdArg arg indent

    | _ ->
      let deepIndent = indent + "  "

      args
      |> List.fold (fun acc arg ->
        acc
        |> cons deepIndent
        |> kdArg arg indent
      ) acc
      |> cons indent

  acc |> cons ")"

let kdNode node indent acc =
  match node with
  | KInt intText ->
    acc |> cons intText

  | KName name ->
    acc |> cons name

  | KPrim (prim, args, res, next) ->
    acc
    |> cons "let "
    |> cons res
    |> cons " = "
    |> cons (kPrimToString prim)
    |> kdArgList args indent
    |> cons indent
    |> kdNode next indent

  | KApp (cal, args) ->
    acc
    |> cons cal
    |> kdArgList args indent

  | KFix (funName, paramList, body, next) ->
    let deepIndent = indent + "  "

    acc
    |> cons "fix "
    |> cons funName
    |> kdParamList paramList indent
    |> cons " {"
    |> cons deepIndent
    |> kdNode body deepIndent
    |> cons indent
    |> cons "}"
    |> cons eol
    |> cons indent
    |> kdNode next indent

let kirDump (node: KNode) =
  []
  |> kdNode node eol
  |> List.rev
  |> String.concat ""