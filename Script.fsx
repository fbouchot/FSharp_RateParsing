#load "Core.fs"
#load "MealPlanParsing.fs"

open Availpro.RateCategorisation.Parsing

MealPlanParsing.parse "Desayuno incluido"

MealPlanParsing.isMatch MealPlanParsing.parseBreakfast "Breakfast included"