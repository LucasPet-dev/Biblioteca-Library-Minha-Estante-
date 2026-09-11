#!/usr/bin/env bash
# Empacota o app como AppImage para Linux x86_64.
# Uso: ./build/linux-appimage/build-linux.sh
#
# Exige appimagetool (download automatico na primeira execucao):
#   https://github.com/AppImage/AppImageKit/releases

set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PROJECT="$ROOT/src/MinhaEstante.Presentation/MinhaEstante.Presentation.csproj"
WORK="$ROOT/build/linux-appimage"
PUBLISH="$WORK/publish/linux-x64"
APP_DIR="$WORK/MinhaEstante.AppDir"
TEMPLATE="$WORK/template"
ICON_SRC="$ROOT/src/MinhaEstante.Presentation/Assets/Logo.png"
OUTPUT="$WORK/MinhaEstante-x86_64.AppImage"

echo "Publicando para linux-x64..."
dotnet publish "$PROJECT" -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o "$PUBLISH"

rm -rf "$APP_DIR"
mkdir -p "$APP_DIR/usr/bin"

cp -r "$PUBLISH"/* "$APP_DIR/usr/bin/"
cp "$TEMPLATE/AppRun" "$APP_DIR/AppRun"
cp "$TEMPLATE/MinhaEstante.desktop" "$APP_DIR/MinhaEstante.desktop"
chmod +x "$APP_DIR/AppRun"

if [ -f "$ICON_SRC" ]; then
  cp "$ICON_SRC" "$APP_DIR/minhaestante.png"
else
  echo "Aviso: icone '$ICON_SRC' nao encontrado; AppImage sera gerada sem icone."
fi

mkdir -p "$WORK/tools"
if [ ! -f "$WORK/tools/appimagetool" ]; then
  echo "Baixando appimagetool..."
  curl -L -o "$WORK/tools/appimagetool" \
    "https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage"
  chmod +x "$WORK/tools/appimagetool"
fi

echo "Gerando AppImage..."
"$WORK/tools/appimagetool" "$APP_DIR" "$OUTPUT"

echo "AppImage gerado: $OUTPUT"