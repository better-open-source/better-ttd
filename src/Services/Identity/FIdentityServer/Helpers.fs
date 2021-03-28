module Helpers

let toOption x =
    if (box x = null) then None
    else Some x