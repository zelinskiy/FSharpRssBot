module SettingsLoader

open System.IO
open System.Collections.Generic
open System.Linq
open System

let loadsettings filepath = 
    filepath
        |> File.ReadLines 
        |> Seq.map (fun s -> s.Split([|" = "|], StringSplitOptions.RemoveEmptyEntries))        
        |> ( fun x -> x.ToDictionary<string[], string, string>((fun p->p.ElementAt(0)), (fun p->p.ElementAt(1))))

let SETTINGS = loadsettings "Configuration.txt"