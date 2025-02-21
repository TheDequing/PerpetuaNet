# PerpetuaNet
Um aplicativo desktop com interface �nica, sincroniza��o P2P via WebRTC e download de torrents via MonoTorrent.

## Funcionalidades
- Interface em uma �nica janela (`MainWindow`) com navega��o por bot�es.
- Sincroniza��o P2P usando WebRTC.NET.
- Download de torrents com MonoTorrent.

## Como executar
1. Clone o reposit�rio: `git clone https://github.com/TheDequing/PerpetuaNet.git`
2. Restaure pacotes: `dotnet restore`
3. Compile e execute: `dotnet run`

## Depend�ncias
- Avalonia 11.2.1
- CommunityToolkit.Mvvm 8.2.2
- WebRTCNET 1.75.0.1-Alpha
- MonoTorrent 2.0.7

## Projeto

Objetivo do Projeto:
Desenvolver um aplicativo que possua:

Sistema central com banco de dados.
Backup local criptografado em cada PC.
Funcionalidade de fallback com sincroniza��o entre os PCs, garantindo a continuidade caso o sistema central falhe.
1. Arquitetura do Sistema
a. Sistema Central
Servidor Central: Hospedado na nuvem (AWS, Google Cloud, ou outro).
Banco de Dados Central: PostgreSQL ou MongoDB.
API RESTful: Para comunica��o entre o app e o servidor central.
Funcionalidades do Sistema Central:
Gerenciamento de contas de usu�rio.
Armazenamento e sincroniza��o de links e dados.
Notifica��es para usu�rios sobre falhas e manuten��o.

b. Sistema Local (Fallback)
Banco de Dados Local: SQLite com SQLCipher (para criptografia).
Backup Local: Cada PC ter� uma c�pia criptografada do banco de dados.
Sincroniza��o com Sistema Central: Sempre que o sistema central estiver acess�vel, os dados locais ser�o sincronizados com o servidor.

2. Funcionalidade de Fallback (Descentraliza��o)
a. Comunica��o entre PCs (P2P)
Tecnologia P2P: WebRTC ou libp2p para comunica��o direta entre PCs.
Tabela de N�s: Cada PC manter� uma lista de n�s (outros PCs) conhecidos.
Sincroniza��o de Dados:
Quando um n� recebe uma atualiza��o de banco de dados, ele propaga para outros n�s.
Uso de CRDTs ou Timestamps para resolu��o de conflitos e consist�ncia de dados.

b. Armazenamento e Criptografia
Criptografia Local: O banco de dados local ser� criptografado com SQLCipher.
Autentica��o: Cada PC utilizar� autentica��o forte para acessar dados, incluindo login e senha.
Localiza��o do Banco: O banco ser� salvo em locais seguros, como C:\ProgramData\ no Windows ou ~/.config/ no Linux, e o nome ser� ofuscado.

c. Detec��o de Falhas
Monitoramento do Sistema Central:
Verifica��o cont�nua da conectividade com o servidor central.
Caso o servidor falhe, o sistema muda automaticamente para o modo offline e usa o banco de dados local.

Recupera��o:
Quando a conex�o for restaurada, os dados locais ser�o sincronizados com o servidor.

3. Funcionalidades de Sincroniza��o
Modo Offline: O app funcionar� normalmente usando dados locais caso o servidor central falhe.
Sincroniza��o Autom�tica:
Quando o servidor central estiver dispon�vel novamente, as altera��es do banco local ser�o enviadas.

A sincroniza��o ser� feita em segundo plano, sem interven��o do usu�rio.
Resolu��o de Conflitos: Durante a sincroniza��o, dados duplicados ou conflitantes ser�o resolvidos com base em timestamps.

4. Seguran�a
a. Criptografia de Dados
Criptografia de Tr�nsito: Toda comunica��o entre o app e o servidor ser� protegida com TLS.
Criptografia Local: O banco de dados local ser� criptografado com AES-256.

b. Prote��o contra Acesso N�o Autorizado
Chave de Criptografia: Ser� armazenada de maneira segura (ex.: usando o Gerenciador de Senhas do Sistema Operacional).
Lista de Confian�a: Conex�es P2P s� ser�o permitidas entre PCs autenticados.

5. Tecnologias e Ferramentas
Backend Central:
Servidor: Node.js ou Python (Django/FastAPI).
Banco de Dados: PostgreSQL ou MongoDB.
API: RESTful usando Express.js (Node.js) ou FastAPI (Python).
Aplicativo Local (Frontend/Backend Local):
Tecnologia de Aplicativo: Electron.js para desenvolvimento de apps desktop ou Python com Tkinter/PyQt.
Banco de Dados Local: SQLite com SQLCipher para criptografia.
Comunica��o P2P: WebRTC ou libp2p.

Outras Ferramentas:
Sincroniza��o: Gun.js ou OrbitDB para facilitar a sincroniza��o entre PCs.
Seguran�a: Libs como crypto (Node.js) ou pycryptodome (Python) para criptografia.

6. Cronograma de Desenvolvimento
Fase 1: Planejamento e Arquitetura
Dura��o: 2 semanas
Definir a arquitetura detalhada do servidor central e do banco local.
Planejar a comunica��o P2P entre os n�s.
Escolher as ferramentas e tecnologias.
Fase 2: Desenvolvimento do Sistema Central
Dura��o: 3 semanas
Configurar o servidor central e o banco de dados.
Criar as APIs RESTful para sincroniza��o de dados e comunica��o com o app.
Implementar a l�gica de autentica��o e gerenciamento de usu�rios.
Fase 3: Desenvolvimento do Banco Local Criptografado
Dura��o: 2 semanas
Implementar o banco local com SQLite e criptografia SQLCipher.
Configurar backup local e l�gica de fallback.
Fase 4: Implementa��o de Comunica��o P2P
Dura��o: 3 semanas
Implementar a comunica��o entre PCs usando WebRTC ou libp2p.
Desenvolver a l�gica de sincroniza��o e propaga��o de dados.
Testar a troca de dados entre dois ou mais PCs em rede.
Fase 5: Testes de Fallback e Sincroniza��o
Dura��o: 2 semanas
Testar cen�rios de falha do servidor central e fallback para banco local.
Testar a sincroniza��o de dados entre os PCs.
Resolver conflitos e ajustar a l�gica de sincroniza��o.
Fase 6: Implementa��o de Seguran�a e Backup
Dura��o: 2 semanas
Implementar criptografia de dados em tr�nsito e local.
Garantir seguran�a na comunica��o entre os n�s e autentica��o.
Fase 7: Testes Finais e Ajustes
Dura��o: 2 semanas
Testar todos os casos de uso.
Ajustar bugs e melhorar a experi�ncia do usu�rio.
Fase 8: Lan�amento
Dura��o: 1 semana
Preparar o release final.
Criar o instalador e distribuir para os usu�rios.
7. Recursos e Custos
Servidores e Hospedagem: AWS, Google Cloud ou DigitalOcean para o servidor central.
Licen�as e Ferramentas: Ferramentas de desenvolvimento, bibliotecas e frameworks.
Manuten��o: Monitoramento do servidor, atualiza��es do app e gerenciamento de dados.




1. Funcionalidade de Download de Torrents
a. Escolha da Biblioteca para Torrent
LibTorrent (libtorrent-rasterbar): Biblioteca popular que pode ser embutida em aplicativos para gerenciar downloads de torrents.
Qbittorrent (Qt + libtorrent): Outra op��o com uma interface gr�fica simples e eficiente, tamb�m com base em libtorrent.
b. Implementa��o no App
Backend Local:

O app far� uso da biblioteca escolhida para iniciar, gerenciar e monitorar o download de torrents.
Cada link de torrent inserido no app ser� baixado usando essa biblioteca.
O download pode ser monitorado atrav�s da interface do app, com indicadores de progresso.
Funcionalidade Adicional:

O app permitir� que o usu�rio adicione links de torrents manualmente ou atrav�s de uma busca.
Poder� configurar op��es como a pasta de download, n�mero de conex�es, limites de velocidade, etc.
c. Gerenciamento de Torrents
Monitoramento: Exibi��o de progresso (barra de progresso, % conclu�do, tempo restante).
Controle: Op��es para pausar, continuar ou cancelar o download.
Armazenamento: Os arquivos baixados ser�o salvos no local especificado pelo usu�rio no app.
d. Interface do Usu�rio
Tela para gerenciar downloads, com a possibilidade de ver detalhes de cada torrent (tamanho, n�mero de arquivos, etc).
O design pode incluir �cones para visualizar torrents ativos, pausados, ou completados.


2. Considera��es T�cnicas e Funcionais
a. Adi��o de Links de Torrent
O usu�rio poder� adicionar um link para o arquivo .torrent ou magnet link diretamente no aplicativo.
O aplicativo buscar� o arquivo correspondente e come�ar� o download imediatamente.

b. Integra��o com o Banco Local
Sincroniza��o de Estado: O status de cada download ser� salvo no banco local (SQLite) para que o progresso seja preservado, mesmo ap�s o app ser fechado e reaberto.
O banco de dados local pode armazenar:
Link do torrent.
Progresso de download.
Status do download (em andamento, pausado, conclu�do).

c. Criptografia de Dados
Seguran�a no Armazenamento: Os arquivos baixados podem ser criptografados localmente usando t�cnicas de criptografia, como AES, se necess�rio.
Criptografia de Tr�nsito: Toda comunica��o com o servidor central e a rede P2P ser� protegida por TLS.


4. Recursos e Tecnologias
Frontend: O frontend pode ser feito com Electron.js ou PyQt, fornecendo uma interface gr�fica para o gerenciamento de torrents.
Backend Local: O backend local pode ser desenvolvido com Python ou Node.js, integrando-se com a biblioteca de torrents.
Biblioteca de Torrent:
LibTorrent (para C++ e Python) ou qbittorrent (para C++/Qt).
Armazenamento Local: Banco de dados local para armazenar informa��es sobre os downloads em andamento e completos.

5. Cronograma de Implementa��o
Fase 1: Planejamento
Definir funcionalidades (quais op��es ser�o disponibilizadas ao usu�rio).
Escolher a biblioteca de torrent a ser integrada.
Fase 2: Implementa��o do Download de Torrents
Integrar a biblioteca de torrent no app.
Criar a interface para gerenciamento de downloads.
Armazenar e monitorar o progresso no banco de dados local.
Fase 3: Testes
Testar o download e controle de torrents em diversos cen�rios (diferentes tipos de links e conex�es).
Testar sincroniza��o de status entre o servidor central e os bancos locais dos usu�rios.
Fase 4: Ajustes e Lan�amento
Ajustar a interface e corrigir bugs encontrados.
Lan�amento final e monitoramento de uso.