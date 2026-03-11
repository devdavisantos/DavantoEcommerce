export default function ProductPage({ params }) {
  const { slug } = params;

  return <div>Produto: {slug}</div>;
}