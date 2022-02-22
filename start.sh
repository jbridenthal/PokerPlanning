#!/bin/bash

set -e
run_cmd="dotnet dev-certs https --trust"
#run_cmd="dotnet run --urls= http://localhost:5000 --project ./PokerPlanning/Server"
run_cmd="dotnet ./out/PokerPlanning.Server.dll"


exec $run_cmd