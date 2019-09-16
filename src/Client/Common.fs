module App.Common

open Fable.React
open Fulma

let selectButton isSelected onClick txt =
  Button.button
    [ Button.OnClick onClick; Button.IsActive isSelected ]
    [ str txt ]

let button onClick txt = selectButton false onClick txt
