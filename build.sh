#!/bin/sh
if [ -z "$VINTAGE_STORY_MODS" ] || [ ! -d "$VINTAGE_STORY_MODS" ]; then
    echo "VINTAGE_STORY_MODS undefined or invalid directory."
    read -p "Vintage Story mods path: " VINTAGE_STORY_MODS

    if [ ! -d "$VINTAGE_STORY_MODS" ]; then
        mkdir -p "$VINTAGE_STORY_MODS"
    fi
fi

dotnet run --project ./CakeBuild/CakeBuild.csproj -- "$@"
rm -rf "$VINTAGE_STORY_MODS/levelup"
cp -r ./Releases/levelup "$VINTAGE_STORY_MODS"