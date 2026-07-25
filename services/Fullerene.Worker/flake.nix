{
  description = "Fullerene Worker Environment";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-25.11";
  };

  outputs = { self, nixpkgs }:
    let
      system = "x86_64-linux";
      pkgs = nixpkgs.legacyPackages.${system};
    in {
      devShells.${system}.worker = pkgs.mkShell {
        buildInputs =[
          pkgs.dotnet-sdk_10
          pkgs.podman
          pkgs.bash
          pkgs.git
        ];
      };
    };
}
