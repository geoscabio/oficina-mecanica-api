# ADR-0009 — Topologia de rede AWS: VPC, 2 AZs, subnets públicas/privadas e NAT Gateway único

## Status

**Status:** ✅ Aceito
**Data:** 13/07/2026
**Autores:** Gabriel de Sousa Silva, Geovanna Monteiro Scabio

---

## 1. Contexto e Problema

> A infraestrutura AWS precisa de uma rede isolada (VPC) que hospede o cluster EKS e o banco RDS com segurança, seguindo o requisito de alta disponibilidade que a própria AWS exige de alguns serviços (EKS e subnet group do RDS pedem subnets em pelo menos duas zonas de disponibilidade), mas o ambiente é temporário, de laboratório, e recriado a cada demonstração — não pode ter custo desnecessário.

## 2. Fatores Decisivos (Drivers)

- **Exigência técnica da AWS:** EKS e o subnet group do RDS exigem subnets distribuídas em no mínimo duas Availability Zones (AZs), mesmo que os recursos reais rodem em apenas uma.
- **Separação público/privado:** recursos que não devem ser expostos diretamente à internet (node do EKS, RDS) precisam ficar em subnets privadas.
- **Custo:** cada NAT Gateway é cobrado por hora, mesmo ocioso — ter um por AZ dobraria o custo sem necessidade real para o volume de tráfego de uma demonstração acadêmica.

## 3. Decisão Proposta

> VPC `10.0.0.0/16`, com subnets em duas AZs (`us-east-1a`, `us-east-1b`): públicas `10.0.1.0/24`/`10.0.2.0/24` e privadas `10.0.11.0/24`/`10.0.12.0/24`. Um único **NAT Gateway compartilhado**, criado apenas na subnet pública de `us-east-1a`, atende a saída para internet de ambas as subnets privadas. Duas route tables (pública → Internet Gateway, privada → NAT Gateway), cada uma associada às duas subnets do seu tipo.

## 4. Justificativa

- A estrutura de subnets nas duas AZs satisfaz a exigência da AWS para EKS e RDS, mesmo com os recursos reais concentrados em uma única zona (1 node EKS, RDS single-AZ — ver ADR-0007 e ADR-0010).
- Um NAT Gateway único reduz o custo pela metade em comparação a um NAT por AZ, aceitável porque a perda de resiliência (se a AZ do NAT cair, a saída para internet das duas subnets privadas para) não é crítica para uma demonstração de curta duração.
- Route tables compartilhadas (uma pública, uma privada) evitam duplicar a mesma regra de rota subnet a subnet.

## 5. Consequências (Trade-offs)

### ✅ Positivo (Ganhos)

- Custo de NAT Gateway reduzido pela metade frente a um design com alta disponibilidade completa.
- Estrutura de rede ainda tecnicamente correta e compatível com os requisitos de EKS/RDS.
- Rede totalmente reproduzível via Terraform, criada e destruída em cada ciclo de demonstração.

### ❌ Negativo (Perdas/Riscos)

- Não é resiliente a falha de zona: se `us-east-1a` ficar indisponível, as subnets privadas de `us-east-1b` perdem acesso à internet (não afeta tráfego interno da VPC, como o node do EKS falando com o RDS).
- Tráfego de saída de recursos eventualmente hospedados em `us-east-1b` atravessa a AZ até o NAT em `us-east-1a`, adicionando uma latência mínima e custo de transferência entre AZs (desprezível no volume desta demonstração).
- Um design de produção real deveria ter um NAT Gateway por AZ.

## 6. Referências

- **AWS.** *Amazon VPC User Guide — NAT Gateways*. 2026.
- **AWS.** *Amazon EKS — VPC and subnet requirements*. 2026.
- Diagrama de referência: [`docs/architecture/diagrams/aws/`](../../architecture/diagrams/aws/).
