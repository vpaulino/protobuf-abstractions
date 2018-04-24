#!/usr/bin/env bash

##########################################################################
# This is the Cake bootstrapper script for Linux and OS X.
# This file was downloaded from https://github.com/cake-build/resources
# Feel free to change this file to fit your needs.
##########################################################################

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
CAKE_EXE=$TOOLS_DIR/Cake/Cake.exe
PACKAGES_CONFIG=$TOOLS_DIR/packages.config
PACKAGES_CONFIG_MD5=$TOOLS_DIR/packages.config.md5sum

# Define md5sum or md5 depending on Linux/OSX
MD5_EXE=
if [[ "$(uname -s)" == "Darwin" ]]; then
    MD5_EXE="md5 -r"
else
    MD5_EXE="md5sum"
fi

# Define default arguments.
SCRIPT="build.cake"
TARGET="Default"
CONFIGURATION="Release"
VERBOSITY="verbose"
DRYRUN=
SHOW_VERSION=false
SCRIPT_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        -s|--script) SCRIPT="$2"; shift ;;
        -t|--target) TARGET="$2"; shift ;;
        -c|--configuration) CONFIGURATION="$2"; shift ;;
        -v|--verbosity) VERBOSITY="$2"; shift ;;
        -d|--dryrun) DRYRUN="-dryrun" ;;
        --version) SHOW_VERSION=true ;;
        --) shift; SCRIPT_ARGUMENTS+=("$@"); break ;;
        *) SCRIPT_ARGUMENTS+=("$1") ;;
    esac
    shift
done

# Make sure the tools folder exist.
if [ ! -d "$TOOLS_DIR" ]; then
  mkdir "$TOOLS_DIR"
fi

# Make sure that packages.config exist.
if [ ! -f "$TOOLS_DIR/packages.config" ]; then
    echo "Downloading packages.config..."
    curl -Lsfo "$TOOLS_DIR/packages.config" http://cakebuild.net/download/bootstrapper/packages
    if [ $? -ne 0 ]; then
        echo "An error occured while downloading packages.config."
        exit 1
    fi
fi

# Restore tools from NuGet.
pushd "$TOOLS_DIR" >/dev/null
if [ ! -f $PACKAGES_CONFIG_MD5 ] || [ "$( cat $PACKAGES_CONFIG_MD5 | sed 's/\r$//' )" != "$( $MD5_EXE $PACKAGES_CONFIG | awk '{ print $1 }' )" ]; then
    find . -type d ! -name . | xargs rm -rf
fi

cd "$TOOLS_DIR"
nuget install -ExcludeVersion -OutputDirectory .
if [ $? -ne 0 ]; then
    echo "Could not restore NuGet packages."
    exit 1
fi

cd "$SCRIPT_DIR"


$MD5_EXE $PACKAGES_CONFIG | awk '{ print $1 }' >| $PACKAGES_CONFIG_MD5

popd >/dev/null

# Make sure that Cake has been installed.
if [ ! -f "$CAKE_EXE" ]; then
    echo "Could not find Cake.exe at '$CAKE_EXE'."
    exit 1
fi

chmod u+x $CAKE_EXE

# Set the mono framework path.
export FrameworkPathOverride=$(dirname $(which mono))/../lib/mono/4.5/

# Start Cake
if $SHOW_VERSION; then
    exec mono "$CAKE_EXE" -version
else
    exec mono "$CAKE_EXE" $SCRIPT -verbosity=$VERBOSITY -configuration=$CONFIGURATION -target=$TARGET $DRYRUN "${SCRIPT_ARGUMENTS[@]}"
fi