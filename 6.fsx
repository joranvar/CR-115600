open System.IO
open System.Text.RegularExpressions

let filereadlines f = File.ReadAllLines (Path.Combine(__SOURCE_DIRECTORY__, f))

type Switch = On | Off | Brightness of int

type Action = On | Off | Toggle

type MessageLanguage = Nordic | English

type Instruction = {
  Operation: Action
  StartRow: int
  StartCol: int
  EndRow: int
  EndCol: int
}

let toggle = function
            | Switch.On -> Switch.Off
            | _         -> Switch.On

let getInstruction (line: string) =
    let matches =
        Regex.Matches(line, "[\w\d_]+")
        |> Seq.cast<Match>
        |> Seq.filter (fun f -> f.Success) |> Seq.map(fun f-> f.Value)
        |> Seq.toList

    let operation, [startRow; startCol; _; endRow; endCol] =
        match matches with
            | "toggle" ::lights          -> Toggle, lights
            | "turn"   ::"on"   ::lights -> On, lights
            | "turn"   ::"off"  ::lights -> Off, lights
            | _                          -> failwith "Bad instruction format"

    { Operation = operation; StartRow = int startRow; StartCol = int startCol; EndRow = int endRow; EndCol = int endCol }

let translateCode (lang: MessageLanguage) (state: Switch) (action: Action) =
    match lang, action, state with
        | English, Toggle, _             -> toggle state
        | English, On,     _             -> Switch.On
        | Nordic,  Toggle, Brightness(x) -> Brightness (x+2)
        | Nordic,  On,     Brightness(x) -> Brightness (x+1)
        | Nordic,  Off,    Brightness(x) -> Brightness (max (x-1) 0)
        | _,       _,      _             -> Switch.Off

let followInstructions (language: MessageLanguage) (lights: Switch[,]) =
    let translate = translateCode language
    "6.txt"
    |> filereadlines
    |> Seq.map getInstruction
    |> Seq.iter(fun ins ->
        for i in ins.StartRow .. ins.EndRow do
            for j in ins.StartCol .. ins.EndCol do
                lights.[i, j] <- translate lights.[i, j] ins.Operation)
    lights

let lights = Array2D.create 1000 1000 Switch.Off
lights
|> followInstructions English
|> Seq.cast<Switch>
|> Seq.filter(fun f -> f = Switch.On)
|> Seq.length
|> printfn "The number of lights that are turned on are %i"

let nordiclights = Array2D.create 1000 1000 (Brightness 0)
nordiclights
|> followInstructions Nordic
|> Seq.cast<Switch>
|> Seq.map(fun f -> match f with
                        | Brightness(x) -> x
                        | _             -> 0)
|> Seq.sum
|> printfn "The Santa's real nordic decoded message and the total brightness is %i"
