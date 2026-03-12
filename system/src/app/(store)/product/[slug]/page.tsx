export default function ProductPage({ params }: { params: { slug: string } }) {
  const { slug } = params;

  return <div>Produto: {slug}</div>;
}