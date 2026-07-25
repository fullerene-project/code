{
  description = "Fullerene Signer Environment";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-25.11";
    android-nixpkgs.url = "github:tadfisher/android-nixpkgs";
  };

  outputs = { self, nixpkgs, android-nixpkgs }:
    let
      system = "x86_64-linux";
      pkgs = nixpkgs.legacyPackages.${system};
      
      android-sdk = android-nixpkgs.sdk.${system} (sdkPkgs: with sdkPkgs;[
        cmdline-tools-latest
        build-tools-35-0-0
      ]);
    in {
      devShells.${system}.signer = pkgs.mkShell {
        buildInputs =[
          pkgs.dotnet-sdk_10
          pkgs.apksigner
          android-sdk
          pkgs.jdk21_headless
          pkgs.bash
        ];

        shellHook = ''
          export ANDROID_SDK_ROOT="${android-sdk}/share/android-sdk"
          export PATH="$ANDROID_SDK_ROOT/build-tools/35.0.0:$PATH"
          
          echo "[Nix Signer Env] zipalign location: $(which zipalign)"
        '';
      };
    };
}
