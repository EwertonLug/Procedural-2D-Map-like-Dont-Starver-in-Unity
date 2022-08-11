# Procedural-2D-Map-like-Dont-Starver-in-Unity

Este projeto é resultado de um estudo sobre Geração Procedimental de Mapas 2D


**Objetivo do Projeto:**

Sempre quis fazer algo procedural e decidi fazer um mapa procedural similar ao "Don't Starver". 
Depois de muito pesquisa e dor de cabeça consegui um resultado até que interessante:

# Como Usar

1. Criar um `TileData`: é aqui que é aqui que é feita a configuração do Tilemaking e outras configurações do Tile.

2. Criar um `BiomeData`, informar um Id e anexar os TileData. Os tiles são selecionados com base do valor de Perlin Noise, o que quer dizer que são selecionados pelo seu index na Lista. 

1. Anexar o BiomeData no `MapGenerator`.

# Ténicas utilizadas:

**Perlin Noise: ** Usado para instanciar os Tiles dos biomas. Foi criada uma função para converte o valor de PerlinNoise para Id (`GetIdUsingPerlin`)


**RRT Algorithm(Adaptado):** Usado para criar os seeds dos Biomas.

**Diagrama de Voronoi: ** Usado para preencher os biomas.

**Tilemasking:** Usado para saber qual tile instanciar com base nos vizinhos.


