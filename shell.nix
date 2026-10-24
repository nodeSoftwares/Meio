# I fixed the deprecated warning but flakes sucks
{ pkgs ? import <nixpkgs> {} }:

let
  libs = with pkgs; [
    fontconfig
    freetype
    libGL
    glib

    # X11 package to render app if you don't use wayland
    libX11
    libICE
    libSM
    libXext
    libXrender
    libxcb
    libxkbcommon

    # lib used to play song
    libvlc

    # install sdk if you don't already have dotnet 10 installed
    dotnet-sdk_10
  ];
in

# use the dotnet package if it's already installed
pkgs.mkShell {
  packages = [ pkgs.dotnet-sdk_10 ] ++ libs;

  LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath libs;
}
