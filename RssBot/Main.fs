module Main

open Rss
open TelegramBot

open System

[<EntryPoint>]
let main argv =  
    test 0    
    Console.ReadLine() |> ignore
    0

(*
TODO:
-/news n command
-subscribe/unsubscribe
-Save users subscriptions as json (mb SQL)
*)