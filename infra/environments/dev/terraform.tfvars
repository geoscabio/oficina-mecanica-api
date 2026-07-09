aws_region = "us-east-1"

# AWS Academy: nao versionar senha real.
# Configure antes de `terraform plan/apply`:
# PowerShell: $env:TF_VAR_db_password = "<senha-forte>"
# Bash: export TF_VAR_db_password="<senha-forte>"

# AWS Academy: os nomes das roles EKS podem mudar entre sessoes/labs.
# Configure antes de `terraform plan/apply`:
# PowerShell:
# $env:TF_VAR_eks_cluster_role_name = "<LabEksClusterRole-...>"
# $env:TF_VAR_eks_node_role_name = "<LabEksNodeRole-...>"
# Bash:
# export TF_VAR_eks_cluster_role_name="<LabEksClusterRole-...>"
# export TF_VAR_eks_node_role_name="<LabEksNodeRole-...>"
