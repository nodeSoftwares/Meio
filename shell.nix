# I know this shit is deprecated but it work so i don't touch it
# I fixed the deprecated warning because flakes sucks
{ pkgs ? import <nixpkgs> {} }:

let
  libs = with pkgs; [
    fontconfig
    freetype
    libGL
    glib
    libX11
    libICE
    libSM
    libXext
    libXrender
    libxcb
    libxkbcommon
    libvlc
    dotnet-sdk_10
  ];
in
pkgs.mkShell {
  packages = [ pkgs.dotnet-sdk_10 ] ++ libs;

  LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath libs;
}
