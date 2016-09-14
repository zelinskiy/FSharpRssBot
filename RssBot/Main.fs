module Main

open Rss
open TelegramBot
open Database

open System

[<EntryPoint>]
let main argv =  
    test 0
    //getNews 100 |> printfn "%A"
    Console.ReadLine() |> ignore
    0

(*
TODO:
-subscribe/unsubscribe
- FIX (Responce only from single host) probelm with "http://feeds.bbci.co.uk/news/rss.xml"
*)