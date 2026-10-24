# I know this shit is deprecated but it work so i don't touch it
{ pkgs ? import <nixpkgs> {} }:

let
  libs = with pkgs; [
    fontconfig
    freetype
    libGL
    glib
    xorg.libX11
    xorg.libICE
    xorg.libSM
    xorg.libXext
    xorg.libXrender
    xorg.libxcb
    libxkbcommon
    libvlc
    dotnet-sdk_10
  ];
in
pkgs.mkShell {
  packages = [ pkgs.dotnet-sdk_10 ] ++ libs;

  LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath libs;
}
