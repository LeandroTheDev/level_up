dotnet run --project ./CakeBuild/CakeBuild.csproj -- "$@"
rm -rf "$VINTAGE_STORY/Mods/levelup"
cp -r ./Releases/levelup "$VINTAGE_STORY/Mods/levelup"